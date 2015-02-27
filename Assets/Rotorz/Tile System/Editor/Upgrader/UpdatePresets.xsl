<?xml version="1.0" encoding="UTF-8"?>
<!-- (c) 2010-2013 Rotorz Limited. All rights reserved. -->
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

	<xsl:output indent="yes" />

	<xsl:template match="preferences">
		<presets xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
			<xsl:apply-templates select="preset"/>
		</presets>
	</xsl:template>
	
	<xsl:template match="preset">
		<preset name="{@name}">
			<xsl:if test="@system-name">
				<SystemName><xsl:value-of select="@system-name"/></SystemName>
			</xsl:if>
			
			<xsl:if test="@tile-width">
				<TileWidth><xsl:value-of select="@tile-width"/></TileWidth>
			</xsl:if>
			<xsl:if test="@tile-height">
				<TileHeight><xsl:value-of select="@tile-height"/></TileHeight>
			</xsl:if>
			<xsl:if test="@tile-depth">
				<TileDepth><xsl:value-of select="@tile-depth"/></TileDepth>
			</xsl:if>
			
			<xsl:if test="@rows">
				<Rows><xsl:value-of select="@rows"/></Rows>
			</xsl:if>
			<xsl:if test="@columns">
				<Columns><xsl:value-of select="@columns"/></Columns>
			</xsl:if>
			
			<xsl:if test="@chunk-width">
				<ChunkWidth><xsl:value-of select="@chunk-width"/></ChunkWidth>
			</xsl:if>
			<xsl:if test="@chunk-height">
				<ChunkHeight><xsl:value-of select="@chunk-height"/></ChunkHeight>
			</xsl:if>
			
			<xsl:if test="@tiles-facing">
				<TilesFacing><xsl:value-of select="@tiles-facing"/></TilesFacing>
			</xsl:if>
			<xsl:if test="@direction">
				<Direction><xsl:value-of select="@direction"/></Direction>
			</xsl:if>
			
			<xsl:if test="@stripping-preset">
				<StrippingPreset><xsl:value-of select="@stripping-preset"/></StrippingPreset>
			</xsl:if>
			<xsl:if test="@stripping-options">
				<StrippingOptions><xsl:value-of select="@stripping-options"/></StrippingOptions>
			</xsl:if>
			
			<xsl:if test="@combine-method">
				<CombineMethod><xsl:value-of select="@combine-method"/></CombineMethod>
			</xsl:if>
			<xsl:if test="@combine-chunk-width">
				<CombineChunkWidth><xsl:value-of select="@combine-chunk-width"/></CombineChunkWidth>
			</xsl:if>
			<xsl:if test="@combine-chunk-height">
				<CombineChunkHeight><xsl:value-of select="@combine-chunk-height"/></CombineChunkHeight>
			</xsl:if>
			
			<xsl:call-template name="convert-to-bool">
				<xsl:with-param name="element">StaticVertexSnapping</xsl:with-param>
				<xsl:with-param name="value" select="@static-vertex-snapping"/>
			</xsl:call-template>
			<xsl:if test="@vertex-snap-threshold">
				<VertexSnapThreshold><xsl:value-of select="@vertex-snap-threshold"/></VertexSnapThreshold>
			</xsl:if>
			
			<xsl:call-template name="convert-to-bool">
				<xsl:with-param name="element">GenerateSecondUVs</xsl:with-param>
				<xsl:with-param name="value" select="@generate-second-uvs"/>
			</xsl:call-template>
			
			<xsl:call-template name="convert-to-bool">
				<xsl:with-param name="element">PregenerateProcedural</xsl:with-param>
				<xsl:with-param name="value" select="@pregenerate-procedural"/>
			</xsl:call-template>
			
			<xsl:call-template name="convert-to-bool">
				<xsl:with-param name="element">HintEraseEmptyChunks</xsl:with-param>
				<xsl:with-param name="value" select="@hint-erase-empty-chumks"/>
			</xsl:call-template>
			
			<xsl:call-template name="convert-to-bool">
				<xsl:with-param name="element">ApplyRuntimeStripping</xsl:with-param>
				<xsl:with-param name="value" select="@apply-runtime-stripping"/>
			</xsl:call-template>
			
			<xsl:call-template name="convert-to-bool">
				<xsl:with-param name="element">UpdateProceduralAtStart</xsl:with-param>
				<xsl:with-param name="value" select="@update-procedural-at-start"/>
			</xsl:call-template>
			
			<xsl:call-template name="convert-to-bool">
				<xsl:with-param name="element">AddProceduralNormals</xsl:with-param>
				<xsl:with-param name="value" select="@add-procedural-normals"/>
			</xsl:call-template>
		</preset>
	</xsl:template>
	
	<xsl:template name="convert-to-bool">
		<xsl:param name="element"/>
		<xsl:param name="value"/>
		<xsl:if test="$value">
			<xsl:element name="{$element}">
				<xsl:choose>
					<xsl:when test="$value = '1'">
						<xsl:text>true</xsl:text>
					</xsl:when>
					<xsl:otherwise>
						<xsl:text>false</xsl:text>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:element>
		</xsl:if>
	</xsl:template>

</xsl:stylesheet>
